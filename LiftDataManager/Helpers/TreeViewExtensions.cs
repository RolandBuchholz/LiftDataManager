namespace LiftDataManager.Helpers;
public static class TreeViewExtensions
{
    public static void ExpandNodeAllLevel(this TreeView treeView, TreeViewNode? node)
    {
        if (node is null)
        {
            return;
        }
        if (node.HasChildren)
        {
            treeView.Expand(node);
            treeView.UpdateLayout();
            foreach (var child in node.Children)
            {
                if (child.HasChildren)
                {
                    treeView.ExpandNodeAllLevel(child);
                }
            }
        }
    }

    public static void ExpandNodePath(this TreeView treeView, string [] pathArray)
    {
        if (pathArray.Length == 0)
        {
            return;
        }
        var rootNode = treeView.RootNodes.First(x => ((HelpContent)x.Content).Name == pathArray[0]);
        TreeViewNode? currentNode = null;

        for (int i = 0; i < pathArray.Length; i++)
        {
            if (i == 0)
            {
                currentNode = rootNode;
            }
            else
            {
                if (currentNode is null || !currentNode.HasChildren)
                {
                    return;
                }
                currentNode = currentNode.Children.First(x => ((HelpContent)x.Content).Name == pathArray[i]);
            }
            if (currentNode is null)
            {
                return;
            }
            treeView.Expand(currentNode);
            treeView.UpdateLayout();
        }
    }
    public static void CollapseAllLevel(this TreeView treeView)
    {
        var openNodes = new List<TreeViewNode>();
        var rootNodes = treeView.RootNodes;

        foreach (var node in rootNodes)
        {
            if (!node.HasChildren)
            {
                continue;
            }
            if (node.IsExpanded)
            {
                openNodes.Add(node);
            }
            foreach (var child in node.Children)
            {
                openNodes.AddRange(GetOpenChildNodes(child));
            }
        }
        openNodes.Reverse();
        foreach (var openNode in openNodes)
        {
            treeView.Collapse(openNode);
        }
    }

    private static List<TreeViewNode> GetOpenChildNodes(TreeViewNode node)
    {
        var openNodes = new List<TreeViewNode>();
        if (node.IsExpanded)
        {
            openNodes.Add(node);
        }
        foreach (var child in node.Children)
        {
            if (child.HasChildren)
            {
                foreach (var subChild in child.Children)
                {
                    openNodes.AddRange(GetOpenChildNodes(subChild));
                }
            }
        }
        return openNodes;
    }
}
