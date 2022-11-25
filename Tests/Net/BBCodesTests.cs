﻿namespace Ecng.Tests.Net
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;

	using Ecng.Common;
	using Ecng.Net.BBCodes;
	using Ecng.UnitTesting;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class BBCodesTests
	{
		private class TextBB2HtmlContext : BB2HtmlContext
		{
			public TextBB2HtmlContext(bool preventScaling, bool allowHtml, string scheme, string domainCode, bool localhost)
				: base(preventScaling, allowHtml, scheme)
			{
				DomainCode = domainCode;
				Localhost = localhost;
			}

			public string DomainCode { get; }
			public bool Localhost { get; }

			public override string GetLocString(string key)
			{
				return key switch
				{
					"ShowSpoiler" => "Show spoiler",
					"Code" => "Code",
					"GoTo" => "Go to",
					_ => throw new ArgumentOutOfRangeException(key),
				};
			}
		}

		private class NamedObjectImpl : INamedObject<TextBB2HtmlContext>
		{
			public long Id { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }

			public string UrlPart { get; set; }

			ValueTask<string> INamedObject<TextBB2HtmlContext>.GetName(TextBB2HtmlContext ctx, CancellationToken token) => new(Name);
			ValueTask<string> INamedObject<TextBB2HtmlContext>.GetDescription(TextBB2HtmlContext ctx, CancellationToken token) => new(Description);
			ValueTask<string> INamedObject<TextBB2HtmlContext>.GetUrlPart(TextBB2HtmlContext ctx, CancellationToken token) => new(UrlPart);
		}

		private class RoleRule : BaseReplaceRule<TextBB2HtmlContext>
		{
			private readonly Regex _regex;
			private readonly Func<long, bool> _isInRole;

			public RoleRule(Func<long, bool> isInRole)
			{
				_regex = new Regex(@"\[role=(?<id>([0-9]*))\](?<inner>(.|\n)*)\[/role\]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
				_isInRole = isInRole ?? throw new ArgumentNullException(nameof(isInRole));
			}

			public override ValueTask<string> ReplaceAsync(TextBB2HtmlContext context, string text, IReplaceBlocks replacement, CancellationToken cancellationToken)
			{
				var builder = new StringBuilder(text);

				for (var match = _regex.Match(text); match.Success; match = _regex.Match(builder.ToString()))
				{
					var what = string.Empty;

					if (long.TryParse(match.Groups["id"].Value, out var roleId))
					{
						if (_isInRole(roleId))
							what = match.Groups["inner"].Value;
					}

					var strText = what;
					replacement.ReplaceHtmlFromText(ref strText, cancellationToken);

					var g = match.Groups[0];

					builder.Remove(g.Index, g.Length);
					builder.Insert(g.Index, strText);
				}

				return new(builder.ToString());
			}
		}

		private static TextBB2HtmlContext CreateContext(bool allowHtml = false, bool localhost = false)
			=> new(false, allowHtml, Uri.UriSchemeHttps, "com", localhost);

		private static readonly Regex _isStockSharpCom = new("href=\"(?<http>(http://)|(https://))?(\\w+.)?stocksharp.com", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex _isStockSharpRu = new("href=\"(?<http>(http://)|(https://))?(\\w+.)?stocksharp.ru", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex _isGitHub = new("href=\"https://github.com/stocksharp", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static BB2HtmlFormatter<TextBB2HtmlContext> CreateBBService(Func<long, bool> isInRole = null)
		{
			static ValueTask<string> GetPackageLink(TextBB2HtmlContext ctx, string packageId, CancellationToken token)
			{
				if (packageId.IsEmpty())
					throw new ArgumentNullException(nameof(packageId));

				return new($"https://www.nuget.org/packages/{packageId}/");
			}

			static ValueTask<string> ToFullAbsolute(TextBB2HtmlContext ctx, string virtualPath, CancellationToken token)
			{
				var domain = ctx.Localhost ? "http://localhost/stocksharp" : $"{ctx.Scheme}://stocksharp.{ctx.DomainCode}";

				if (virtualPath.StartsWithIgnoreCase("http"))
				{ }
				else if (virtualPath.StartsWith("/"))
					virtualPath = $"{domain}{virtualPath}";
				else
					virtualPath = virtualPath.Replace("~", domain);

				return new(virtualPath);
			}

			static ValueTask<INamedObject<TextBB2HtmlContext>> GetProduct(long id, CancellationToken token)
			{
				//var idStr = urlPart.IsEmpty() ? id.To<string>() : urlPart.ToLowerInvariant();
				//return $"~/store/{idStr}/";

				return new(id switch
				{
					9 => new NamedObjectImpl { Id = id, Name = "S#.Designer", UrlPart = "~/store/strategy designer/" },
					_ => throw new ArgumentOutOfRangeException(nameof(id)),
				});
			}

			static ValueTask<INamedObject<TextBB2HtmlContext>> GetFile(long id, CancellationToken token)
				=> new(new NamedObjectImpl { Id = id, UrlPart = $"~/file/{id}/" });

			static ValueTask<INamedObject<TextBB2HtmlContext>> GetTopic(long id, CancellationToken token)
				=> new(new NamedObjectImpl { Id = id, UrlPart = $"~/topic/{id}/", Name = $"Test topic {id}" });

			static ValueTask<INamedObject<TextBB2HtmlContext>> GetMessage(long id, CancellationToken token)
				=> new(new NamedObjectImpl { Id = id, UrlPart = $"~/posts/m/{id}/", Name = $"Test message {id}" });

			static ValueTask<INamedObject<TextBB2HtmlContext>> GetUser(long id, CancellationToken token)
			{
				return new(id switch
				{
					1 => new NamedObjectImpl { Id = id, Name = "StockSharp", UrlPart = $"~/users/{id}/", Description = "StockSharp" },
					777 => new NamedObjectImpl { Id = id, Name = "Some user", UrlPart = $"~/users/{id}/", Description = "Some user" },
					_ => throw new ArgumentOutOfRangeException(nameof(id)),
				});
			}

			static ValueTask<INamedObject<TextBB2HtmlContext>> GetPage(long id, CancellationToken token)
			{
				return new(id switch
				{
					239 => new NamedObjectImpl { Id = id, Name = "Using S#.Installer to publish user content", UrlPart = "~/store/faq/" },
					_ => throw new ArgumentOutOfRangeException(nameof(id)),
				});
			}

			var bb = new BB2HtmlFormatter<TextBB2HtmlContext>(
				GetProduct,
				GetUser,
				GetFile,
				GetTopic,
				GetMessage,
				GetPage,

				GetPackageLink,
				(ctx, s, t) => new(s),
				ToFullAbsolute,
				(ctx, sourceUrl, t) => new($"{sourceUrl}_a6f78c5fce344124993798c028a22a3a"),
				(ctx, t) => new($"stocksharp.{ctx.DomainCode}"),
				async (ctx, url, t) =>
				{
					string domain = null;

					var urlStr = url.ToString();

					Match match;

					if ((match = _isStockSharpCom.Match(urlStr)).Success)
						domain = "com";
					else if ((match = _isStockSharpRu.Match(urlStr)).Success)
						domain = "ru";

					var changed = !domain.IsEmpty() && (ctx.Localhost || domain != ctx.DomainCode);
					if (changed)
					{
						url.ReplaceIgnoreCase($"{match.Groups["http"]}stocksharp.{domain}", await ToFullAbsolute(ctx, "~", t));
					}

					var isGitHub = _isGitHub.IsMatch(urlStr);
					var isAway = domain is null && !isGitHub;
					var noFollow = isAway;
					var isBlank = isAway || isGitHub;
					return (changed, isAway, noFollow, isBlank);
				},
				(ctx, url, t) => new(url.UrlEscape()),
				(ctx, img, t) =>
				{
					return new(Path.GetExtension(img).EqualsIgnoreCase(".gif")
						? $"~/images/smiles/{img}"
						: $"~/images/svg/smiles/{img}");
				},
				(ctx, source, error) => throw error);

			if (isInRole is null)
				isInRole = i => true;

			//bb.AddRule(new VideoStockSharpReplaceRule());
			bb.AddRule(new RoleRule(isInRole));

			return bb;
		}

		[TestMethod]
		public async Task Test()
		{
			var bb = @"[h2=text_format]Text format[/h2]
[t=width:100%]
[th]BB code[/th]
[th]Description[/th]
[th]Syntax[/th]
[th]Example[/th]
[tr]
[td]noparse[/td]
[td]Block any BB parsing inside[/td]
[td][noparse][b]This text not parsed[/b][/noparse][/td]
[td][noparse][b]This text not parsed[/b][/noparse][/td]
[/tr]
[tr]
[td]b[/td]
[td]Bold font[/td]
[td][noparse][b]This text is bold[/b][/noparse][/td]
[td][b]This text is bold[/b][/td]
[/tr]
[tr]
[td]i[/td]
[td]Italic font[/td]
[td][noparse][i]This text is italic[/i][/noparse][/td]
[td][i]This text is italic[/i][/td]
[/tr]
[tr]
[td]font[/td]
[td]Font family[/td]
[td][noparse][font=cursive]Cursive text[/font][/noparse][/td]
[td][font=cursive]Cursive text[/font][/td]
[/tr]
[tr]
[td]color[/td]
[td]Text color[/td]
[td][noparse][color=red]Red text[/color][/noparse][/td]
[td][color=red]Red text[/color][/td]
[/tr]
[tr]
[td]size[/td]
[td]Text size[/td]
[td][noparse][size=10]This text has 7 size[/size][/noparse][/td]
[td][size=7]This text has 7 size[/size][/td]
[/tr]
[tr]
[td]highlight[/td]
[td]Text highlight[/td]
[td][noparse][h]This text highlighted[/h][/noparse][/td]
[td][h]This text highlighted[/h][/td]
[/tr]
[/t]

[h2=text_align]Text align[/h2]
[t=width:100%]
[th]BB code[/th]
[th]Description[/th]
[th]Syntax[/th]
[th]Example[/th]
[tr]
[td]float[/td]
[td]Floating text[/td]
[td][noparse][float=right]This text floated to right[/float][/noparse][/td]
[td][float=right]This text floated to right[/float][/td]
[/tr]
[tr]
[td]left[/td]
[td]Align to left[/td]
[td][noparse][left]This text aligned to left[/left][/noparse][/td]
[td][left]This text aligned to left[/left][/td]
[/tr]
[tr]
[td]right[/td]
[td]Align to right[/td]
[td][noparse][right]This text aligned to right[/right][/noparse][/td]
[td][right]This text aligned to right[/right][/td]
[/tr]
[tr]
[td]center[/td]
[td]Align to center[/td]
[td][noparse][center]This text aligned to center[/center][/noparse][/td]
[td][center]This text aligned to center[/center][/td]
[/tr]
[tr]
[td]code[/td]
[td]Code block[/td]
[td][noparse][code]var i = 10;[/code][/noparse][/td]
[td][code]var i = 10;[/code][/td]
[/tr]
[tr]
[td]code[/td]
[td]Code block[/td]
[td][noparse][code=c#]var i = 10;[/code][/noparse][/td]
[td][code=c#]var i = 10;[/code][/td]
[/tr]
[tr]
[td]spoiler[/td]
[td]Collapsed text[/td]
[td][noparse][spoiler]This test is collapsed[/spoiler][/noparse][/td]
[td][spoiler]This test is collapsed[/spoiler][/td]
[/tr]
[/t]

[h2=links]Links[/h2]
[t=width:100%]
[th]BB code[/th]
[th]Description[/th]
[th]Syntax[/th]
[th]Example[/th]
[tr]
[td]url[/td]
[td]Link[/td]
[td][noparse][url]https://stocksharp.com/[/url][/noparse][/td]
[td][url]https://stocksharp.com/[/url][/td]
[/tr]
[tr]
[td]url[/td]
[td]Link with text[/td]
[td][noparse][url=https://stocksharp.com/]StockSharp Web site[/url][/noparse][/td]
[td][url=https://stocksharp.com/]StockSharp Web site[/url][/td]
[/tr]
[tr]
[td]email[/td]
[td]Email link[/td]
[td][noparse][email]info@stocksharp.com[/email][/noparse][/td]
[td][email]info@stocksharp.com[/email][/td]
[/tr]
[tr]
[td]email[/td]
[td]Email link with text[/td]
[td][noparse][email=info@stocksharp.com]Send me[/email][/noparse][/td]
[td][email=info@stocksharp.com]Send me[/email][/td]
[/tr]
[tr]
[td]topic[/td]
[td]Topic link by id[/td]
[td][noparse][topic=12374]Link to topic[/topic][/noparse][/td]
[td][topic=12374]Link to topic[/topic][/td]
[/tr]
[tr]
[td]message[/td]
[td]Message link by id[/td]
[td][noparse][message=51317]Link to message[/message][/noparse][/td]
[td][message=51317]Link to message[/message][/td]
[/tr]
[tr]
[td]product[/td]
[td]Product link by id[/td]
[td][noparse][product]9[/product][/noparse][/td]
[td][product]9[/product][/td]
[/tr]
[tr]
[td]product[/td]
[td]Product link by id with text[/td]
[td][noparse][product=9]Link to S#.Designer[/product][/noparse][/td]
[td][product=9]Link to S#.Designer[/product][/td]
[/tr]
[tr]
[td]user[/td]
[td]User link by id[/td]
[td][noparse][user]1[/user][/noparse][/td]
[td][user]1[/user][/td]
[/tr]
[/t]

[h2=images]Images[/h2]
[t=width:100%]
[th]BB code[/th]
[th]Description[/th]
[th]Syntax[/th]
[th]Example[/th]
[tr]
[td]img[/td]
[td]Image by url[/td]
[td][noparse][img]https://stocksharp.com/file/122179/cup_svg/[/img][/noparse][/td]
[td][img]https://stocksharp.com/file/122179/cup_svg/[/img][/td]
[/tr]
[tr]
[td]img[/td]
[td]Image by url with tooltip[/td]
[td][noparse][img=https://stocksharp.com/file/122179/cup_svg/]This is custom tooltip.[/img][/noparse][/td]
[td][img=https://stocksharp.com/file/122179/cup_svg/]This is custom tooltip.[/img][/td]
[/tr]
[tr]
[td]img[/td]
[td]Image by id[/td]
[td][noparse][img]122179[/img][/noparse][/td]
[td][img]122179[/img][/td]
[/tr]
[tr]
[td]img[/td]
[td]Image by id with tooltip[/td]
[td][noparse][img=122179]This is custom tooltip.[/img][/noparse][/td]
[td][img=122179]This is custom tooltip.[/img][/td]
[/tr]
[tr]
[td]img size[/td]
[td]Image with fixed size[/td]
[td][noparse]
[size width=10px height=10px][img]https://stocksharp.com/file/122179/cup_svg/[/img][/size]
[size width=10px height=10px][img]122179[/img][/size]
[size width=10px height=10px][img=122179]This is custom tooltip.[/img][/size]
[/noparse][/td]
[td]
[size width=10px height=10px][img]https://stocksharp.com/file/122179/cup_svg/[/img][/size]
[size width=10px height=10px][img]122179[/img][/size]
[size width=10px height=10px][img=122179]This is custom tooltip.[/img][/size]
[/td]
[/tr]
[/t]

[h2=lists]Lists[/h2]
[t=width:100%]
[th]BB code[/th]
[th]Description[/th]
[th]Syntax[/th]
[th]Example[/th]
[tr]
[td]list[/td]
[td]List with bullet[/td]
[td][noparse]
[list]
[*]Item 1
[*]Item 2
[*]Item 3
[/list]
[/noparse][/td]
[td]
[list]
[*]Item 1
[*]Item 2
[*]Item 3
[/list]
[/td]
[/tr]
[tr]
[td]list[/td]
[td]List with number[/td]
[td][noparse]
[list=1]
[*]Item 1
[*]Item 2
[*]Item 3
[/list]
[/noparse][/td]
[td]
[list=1]
[*]Item 1
[*]Item 2
[*]Item 3
[/list]
[/td]
[/tr]
[tr]
[td]list[/td]
[td]List with letter[/td]
[td][noparse]
[list=a]
[*]Item 1
[*]Item 2
[*]Item 3
[/list]
[/noparse][/td]
[td]
[list=a]
[*]Item 1
[*]Item 2
[*]Item 3
[/list]
[/td]
[/tr]
[tr]
[td]list[/td]
[td]List with latin[/td]
[td][noparse]
[list=i]
[*]Item 1
[*]Item 2
[*]Item 3
[/list]
[/noparse][/td]
[td]
[list=i]
[*]Item 1
[*]Item 2
[*]Item 3
[/list]
[/td]
[/tr]
[/t]

[h2=social]Social[/h2]
[t=width:100%]
[th]BB code[/th]
[th]Description[/th]
[th]Syntax[/th]
[th]Example[/th]
[tr]
[td]youtube[/td]
[td]YouTube[/td]
[td][noparse][youtube]https://youtu.be/lsbyvqajlZ8[/youtube][/noparse][/td]
[td][youtube]https://youtu.be/lsbyvqajlZ8[/youtube][/td]
[/tr]
[tr]
[td]vimeo[/td]
[td]Vimeo[/td]
[td][noparse][vimeo]https://vimeo.com/60238948[/vimeo][/noparse][/td]
[td][vimeo]https://vimeo.com/60238948[/vimeo][/td]
[/tr]
[tr]
[td]fb[/td]
[td]Facebook[/td]
[td][noparse][fb]https://www.facebook.com/stocksharp/posts/4554987014515465[/fb][/noparse][/td]
[td][fb]https://www.facebook.com/stocksharp/posts/4554987014515465[/fb][/td]
[/tr]
[/t]

[role=133362]
[h2=extended]Extended (allowed only for administrators)[/h2]
[t=width:100%]
[th]BB code[/th]
[th]Description[/th]
[th]Syntax[/th]
[th]Example[/th]
[tr]
[td]page[/td]
[td]Page link by id[/td]
[td][noparse][page=239]Link to store FAQ[/page][/noparse][/td]
[td][page=239]Link to store FAQ[/page][/td]
[/tr]
[tr]
[td]page[/td]
[td]Page link by id[/td]
[td][noparse][page=239][/page][/noparse][/td]
[td][page=239][/page][/td]
[/tr]
[tr]
[td]html[/td]
[td]Embedded html code[/td]
[td][noparse][html]any html code here[/html][/noparse][/td]
[td][html]<blockquote class=""twitter-tweet""><p lang=""en"" dir=""ltr"">Hi all!<br><br>Do you know what benefits and knowledge you can get from our training in algorithmic training course?<br>- Create your own trading robot<br>- Real made working examples of trading strategies<br>- Training chat<br><br>Follow the link below to learn more!<a href=""https://t.co/YpsKldZ2GO"">https://t.co/YpsKldZ2GO</a> <a href=""https://t.co/U1hxgfY3nc"">pic.twitter.com/U1hxgfY3nc</a></p>&mdash; StockSharp (@StockSharp) <a href=""https://twitter.com/StockSharp/status/1383028699780554759?ref_src=twsrc%5Etfw"">April 16, 2021</a></blockquote> <script async src=""https://platform.twitter.com/widgets.js"" charset=""utf-8""></script>[/html][/td]
[/tr]
[tr]
[td]role[/td]
[td]Hidden for public access text[/td]
[td][noparse][role=133362]This text visible for content managers only[/role][/noparse][/td]
[td][role=133362]This text visible for content managers only[/role][/td]
[/tr]
[/t]
[/role]";

			var ctx = CreateContext(true);
			var res = await CreateBBService().ToHtmlAsync(bb, ctx);

			res.AssertEqual(BBHelper.Html);
		}

		[TestMethod]
		public async Task NoHtml()
		{
			var ctx = CreateContext();
			var res = await CreateBBService().ToHtmlAsync(@"[html]<script async src=""https://platform.twitter.com/widgets.js"" charset=""utf-8""></script>[/html]", ctx);

			res.AssertEqual("<span>&lt;script async src=&quot;https://platform.twitter.com/widgets.js&quot; charset=&quot;utf-8&quot;&gt;&lt;/script&gt;</span>");
		}

		[TestMethod]
		public async Task UrlEncode()
		{
			var ctx = CreateContext();
			var svc = CreateBBService();
			var res = await svc.ToHtmlAsync(@"[url]https://stocksharp.com/strategy designer/[/url]", ctx);

			var html = "<a href=\"https://stocksharp.com/strategy%20designer/\" title=\"https://stocksharp.com/strategy%20designer/\">https://stocksharp.com/strategy designer/</a>";
			res.AssertEqual(html);

			res = await svc.ToHtmlAsync(@"[url=https://stocksharp.com/strategy designer/]StockSharp Designer[/url]", ctx);

			html = "<a href=\"https://stocksharp.com/strategy%20designer/\" title=\"https://stocksharp.com/strategy%20designer/\">StockSharp Designer</a>";
			res.AssertEqual(html);
		}

		[TestMethod]
		public async Task Away()
		{
			var ctx = CreateContext();
			var svc = CreateBBService();

			async Task Check(string bb, string html)
			{
				var res = await svc.ToHtmlAsync(bb, ctx);
				res.AssertEqual(html);
			}

			await Check("[url]https://google.com/[/url]", "<a target=\"_blank\" rel=\"nofollow\" href=\"https://stocksharp.com/away/?u=https://google.com/\" title=\"https://google.com/\">https://google.com/</a>");
			await Check("https://google.com/", "<a target=\"_blank\" rel=\"nofollow\" href=\"https://stocksharp.com/away/?u=https://google.com\" title=\"https://google.com\">https://google.com</a>/");
			await Check("[url]https://stocksharp.com/[/url]", "<a href=\"https://stocksharp.com/\" title=\"https://stocksharp.com/\">https://stocksharp.com/</a>");
			await Check("[url=https://stocksharp.ru]StockSharp[/url]", "<a href=\"https://stocksharp.com\" title=\"https://stocksharp.com\">StockSharp</a>");
			await Check("https://crowd.stocksharp.com/", "<a href=\"https://crowd.stocksharp.com\" title=\"https://crowd.stocksharp.com\">https://crowd.stocksharp.com</a>/");
			await Check("[url=https://crowd.stocksharp.com/]crowds[/url]", "<a href=\"https://crowd.stocksharp.com/\" title=\"https://crowd.stocksharp.com/\">crowds</a>");
			await Check("https://github.com/stocksharp/", "<a target=\"_blank\" href=\"https://github.com/stocksharp/\" title=\"https://github.com/stocksharp/\">https://github.com/stocksharp/</a>");
			await Check("https://github.com/STOCKSHARP/", "<a target=\"_blank\" href=\"https://github.com/STOCKSHARP/\" title=\"https://github.com/STOCKSHARP/\">https://github.com/STOCKSHARP/</a>");
			await Check("[url]https://github.com/stocksharp/algo/[/url]", "<a target=\"_blank\" href=\"https://github.com/stocksharp/algo/\" title=\"https://github.com/stocksharp/algo/\">https://github.com/stocksharp/algo/</a>");
			await Check("[url=https://github.com/stocksharp/]github[/url]", "<a target=\"_blank\" href=\"https://github.com/stocksharp/\" title=\"https://github.com/stocksharp/\">github</a>");
			await Check("https://doc.stocksharp.com/", "<a href=\"https://doc.stocksharp.com\" title=\"https://doc.stocksharp.com\">https://doc.stocksharp.com</a>/");
		}

		[TestMethod]
		public async Task Localhost()
		{
			var ctx = CreateContext(false, true);
			var svc = CreateBBService();
			var res = await svc.ToHtmlAsync(@"[url]https://stocksharp.com/strategy designer/[/url]", ctx);

			var html = "<a href=\"http://localhost/stocksharp/strategy%20designer/\" title=\"http://localhost/stocksharp/strategy%20designer/\">http://localhost/stocksharp/strategy designer/</a>";
			res.AssertEqual(html);

			res = await svc.ToHtmlAsync(@"[url=https://stocksharp.com/strategy designer/]StockSharp Designer[/url]", ctx);

			html = "<a href=\"http://localhost/stocksharp/strategy%20designer/\" title=\"http://localhost/stocksharp/strategy%20designer/\">StockSharp Designer</a>";
			res.AssertEqual(html);

			res = await svc.ToHtmlAsync(@"[user]1[/user]", ctx);

			html = "<a href=\"http://localhost/stocksharp/users/1/\" title=\"StockSharp\">StockSharp</a>";
			res.AssertEqual(html);

			res = await svc.ToHtmlAsync(@"[url=https://google.com/]Google[/url]", ctx);

			html = "<a target=\"_blank\" rel=\"nofollow\" href=\"https://stocksharp.com/away/?u=https://google.com/\" title=\"https://google.com/\">Google</a>";
			res.AssertEqual(html);
		}

		[TestMethod]
		public async Task Quote()
		{
			var ctx = CreateContext();
			var svc = CreateBBService();

			var res = await svc.ToHtmlAsync(@"[quote=LTrader;42149]Добрый день, уважаемые разработчики, и коллеги.[/quote] ответ", ctx);

			var html = "<div class=\"quote\"><span class=\"quotetitle\">LTrader <a href=\"https://stocksharp.com/posts/m/42149/\"><img src=\"https://stocksharp.com/images/smiles/icon_latest_reply.gif\" title=\"Go to\" alt=\"Go to\" /></a></span><div class=\"innerquote\">Добрый день, уважаемые разработчики, и коллеги.</div></div> ответ";
			res.AssertEqual(html);
		}

		[TestMethod]
		public async Task Roles()
		{
			var ctx = CreateContext();

			const string txt = @"normal text[role=123] hidden text[/role]";

			var svc = CreateBBService();
			(await svc.ToHtmlAsync(txt, ctx)).AssertEqual("normal text hidden text");
			(await svc.CleanAsync(txt, default)).AssertEqual("normal text hidden text");
			(await svc.ActivateRuleAsync(txt, svc.Rules.First(r => r is RoleRule), default)).AssertEqual("normal text hidden text");

			svc = CreateBBService(i => false);
			(await svc.ToHtmlAsync(txt, ctx)).AssertEqual("normal text");
			//TODO (await svc.CleanAsync(txt, default)).AssertEqual("normal text");
			(await svc.ActivateRuleAsync(txt, svc.Rules.First(r => r is RoleRule), default)).AssertEqual("normal text");
			(await svc.ActivateRuleAsync($"[b]{txt}[/b]", svc.Rules.First(r => r is RoleRule), default)).AssertEqual("[b]normal text[/b]");
		}

		[TestMethod]
		public async Task Links()
		{
			var ctx = CreateContext();
			var svc = CreateBBService();

			async ValueTask Do(string bb, string html)
			{
				var res = await svc.ToHtmlAsync(bb, ctx);
				res.AssertEqual(html);
			}

			await Do("[user=777]Link to user[/user]", "<a href=\"https://stocksharp.com/users/777/\" title=\"Some user\">Link to user</a>");
			await Do("[user=777][/user]", "<a href=\"https://stocksharp.com/users/777/\" title=\"Some user\">Some user</a>");
			await Do("[user]777[/user]", "<a href=\"https://stocksharp.com/users/777/\" title=\"Some user\">Some user</a>");

			await Do("[topic=42149]Link to topic[/topic]", "<a href=\"https://stocksharp.com/topic/42149/\" title=\"Test topic 42149\">Link to topic</a>");
			await Do("[topic=42149][/topic]", "<a href=\"https://stocksharp.com/topic/42149/\" title=\"Test topic 42149\">Test topic 42149</a>");
			await Do("[topic]42149[/topic]", "<a href=\"https://stocksharp.com/topic/42149/\" title=\"Test topic 42149\">Test topic 42149</a>");

			await Do("[post=42149]Link to post[/post]", "<a href=\"https://stocksharp.com/posts/m/42149/\" title=\"Test message 42149\">Link to post</a>");
			await Do("[post=42149][/post]", "<a href=\"https://stocksharp.com/posts/m/42149/\" title=\"Test message 42149\">Test message 42149</a>");
			await Do("[post]42149[/post]", "<a href=\"https://stocksharp.com/posts/m/42149/\" title=\"Test message 42149\">Test message 42149</a>");

			await Do("[message=42149]Link to message[/message]", "<a href=\"https://stocksharp.com/posts/m/42149/\" title=\"Test message 42149\">Link to message</a>");
			await Do("[message=42149][/message]", "<a href=\"https://stocksharp.com/posts/m/42149/\" title=\"Test message 42149\">Test message 42149</a>");
			await Do("[message]42149[/message]", "<a href=\"https://stocksharp.com/posts/m/42149/\" title=\"Test message 42149\">Test message 42149</a>");

			await Do("[page=239]Link to page[/page]", "<a href=\"https://stocksharp.com/store/faq/\" title=\"Using S#.Installer to publish user content\">Link to page</a>");
			await Do("[page=239][/page]", "<a href=\"https://stocksharp.com/store/faq/\" title=\"Using S#.Installer to publish user content\">Using S#.Installer to publish user content</a>");
			await Do("[page]239[/page]", "<a href=\"https://stocksharp.com/store/faq/\" title=\"Using S#.Installer to publish user content\">Using S#.Installer to publish user content</a>");

			await Do("[product=9]Link to product[/product]", "<a href=\"https://stocksharp.com/store/strategy%20designer/\" title=\"S#.Designer\">Link to product</a>");
			await Do("[product=9][/product]", "<a href=\"https://stocksharp.com/store/strategy%20designer/\" title=\"S#.Designer\">S#.Designer</a>");
			await Do("[product]9[/product]", "<a href=\"https://stocksharp.com/store/strategy%20designer/\" title=\"S#.Designer\">S#.Designer</a>");
		}
	}
}
