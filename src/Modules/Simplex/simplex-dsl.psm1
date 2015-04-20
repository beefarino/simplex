
$script:currentParentNode = new-object System.Collections.Stack;

function root {
    param(
        [parameter(position=0, mandatory=$true)]
        [scriptblock]
        # a scriptblock that outputs child folders and script nodes
        $script
    )

    $root = new-object codeowls.ScriptProvider.nodes.rootfolder -arg 'root';
    $script:currentParentNode.Push($root);
    $items = [CodeOwls.ScriptProvider.nodes.iitem[]]@(& $script | ? {$_ -is [codeowls.ScriptProvider.nodes.iitem] });

    $root.children.addrange($items);
    $script:currentParentNode.Pop() | out-null;

    return $root;
<#
   .SYNOPSIS
    Defines the root node of the Simplex-based provider.
   .DESCRIPTION
   Defines the root node of the Simplex-based provider.

   The root node can contain folder nodes and script nodes.

   .EXAMPLE
    root {
      folder "myFolder" {
        script "myScript" {
          get-eventlog -LogName Application -Newest 20 -EntryType error;
        }
      }
    }
#>
}

function script {

    param(
        [parameter(position=0, mandatory=$true)]
        [string]
        # the name of the container
        $name,

        [parameter()]
        [string]
        # the name of the object property to use as the item name for child objects
        $idField,

        [parameter(mandatory=$true, position = 2)]
        [scriptblock]
        # a scriptblock that outputs child objects
        $script
    )

    $menu = new-object 'system.collections.generic.dictionary[string,ScriptBlock]'
    for( $c = 0; $c -lt $contextMenuItems.length; $c += 2 )
    {
        $key = $contextMenuItems[$c].TrimStart('-');
        $sb = $contextMenuItems[1+$c];

        if( $sb -isnot [scriptblock] )
        {
            write-error "context menu item '$key' specifies an invalid value '$sb' of type '$($sb.getType().fullName)'; only scriptblocks may be specified for context menu items";
            return;
        }

        $menu[$key] = [scriptblock]$sb;
    }

    if( $icon -and -not $icon.EndsWith('.ico') -and -not $icon.StartsWith('.') -and -not $icon.Contains(',') ) { $icon = ".$icon" }
    if( $itemicon -and -not $itemicon.StartsWith('.') ) { $itemicon = ".$itemicon" }
    $s = new-object codeowls.ScriptProvider.nodes.scriptfolder -arg $name,$script,$idField;

    return $s;
<#
   .SYNOPSIS
   Defines a script-based container node in the simplex provider.
   .DESCRIPTION
   Defines a script-based container node in the simplex provider.  Script nodes
   are evaluated on-demand as necessary by the Simplex provider.

   The objects output by the script become child objects of this node. Script
   nodes can contain child Folder and Script nodes.  This means you can
   generate dynamic child node hierarchies.

   If an object returned by the script is another provider item, the relevant
   item properties from the original provider are proxied to the simplex
   provider.

   .EXAMPLE
    root {
      folder "myFolder" {
        script "Errors" {
          get-eventlog -LogName Application -Newest 20 -EntryType error;
        }
        script "MyDocuments" {
          dir $home\documents;
        }
      }
    }
#>
}

function folder {

    param(
        [parameter(position=0, mandatory=$true)]
        [string]
        # the name of the container
        $name,

        [parameter(position=1, mandatory=$true)]
        [scriptblock]
        # a scriptblock that outputs child folders and script nodes
        $script
    )

    $folder = new-object codeowls.ScriptProvider.nodes.folder -arg $name;
    $script:currentParentNode.Push($folder);
    $items=[codeowls.ScriptProvider.nodes.iitem[]]@(& $script);
    $folder.Children.AddRange( $items );
    $script:currentParentNode.Pop() | out-null;

    return $folder;
<#
   .SYNOPSIS
   Defines a named container node in the simplex provider.
   .DESCRIPTION
   Defines a named container node in the simplex provider.  Folder nodes
   are evaluated once in the context of the Simplex DSL script load.

   The folder node can contain other folder nodes and script nodes.

   .EXAMPLE
    root {
      folder "myFolder" {
        script "myScript" {
          get-eventlog -LogName Application -Newest 20 -EntryType error;
        }
      }
    }
#>
}
