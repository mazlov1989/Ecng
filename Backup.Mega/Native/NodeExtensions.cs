﻿namespace Ecng.Backup.Mega.Native
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  static class NodeExtensions
  {
    public static long GetFolderSize(this INode node, IEnumerable<INode> allNodes)
    {
      if (node.Type == NodeType.File)
      {
        throw new InvalidOperationException("node is not a Directory");
      }

      long folderSize = 0;
      var children = allNodes.Where(x => x.ParentId == node.Id);
      foreach (var childNode in children)
      {
        if (childNode.Type == NodeType.File)
        {
          folderSize += childNode.Size;
        }
        else if (childNode.Type == NodeType.Directory)
        {
          var size = childNode.GetFolderSize(allNodes);
          folderSize += size;
        }
      }

      return folderSize;
    }

#if !NET40

    public static async Task<long> GetFolderSizeAsync(this INode node, MegaApiClient client, CancellationToken cancellationToken)
    {
      var allNodes = await client.GetNodesAsync(cancellationToken);
      return await node.GetFolderSizeAsync(allNodes);
    }

    public static async Task<long> GetFolderSizeAsync(this INode node, IEnumerable<INode> allNodes)
    {
      if (node.Type == NodeType.File)
      {
        throw new InvalidOperationException("node is not a Directory");
      }

      long folderSize = 0;
      var children = allNodes.Where(x => x.ParentId == node.Id);
      foreach (var childNode in children)
      {
        if (childNode.Type == NodeType.File)
        {
          folderSize += childNode.Size;
        }
        else if (childNode.Type == NodeType.Directory)
        {
          var size = await childNode.GetFolderSizeAsync(allNodes);
          folderSize += size;
        }
      }

      return folderSize;
    }

#endif
  }
}
