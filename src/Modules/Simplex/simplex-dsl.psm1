
$script:currentParentNode = new-object System.Collections.Stack;

function root {
    param(
        [parameter(position=0, mandatory=$true)]
        [scriptblock]
        $script
    )

    $root = new-object codeowls.ScriptProvider.nodes.rootfolder -arg 'root';
    $script:currentParentNode.Push($root);
    $items = [CodeOwls.ScriptProvider.nodes.iitem[]]@(& $script | ? {$_ -is [codeowls.ScriptProvider.nodes.iitem] });
    
    $root.children.addrange($items);
    $script:currentParentNode.Pop() | out-null;

    return $root;
}

function script {
    
    param(
        [parameter(position=0, mandatory=$true)]
        [string]
        $name,
        
        [parameter()]
        [string]
        $idField,

        [parameter(mandatory=$true, position = 2)]
        [scriptblock]
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
    $s = new-object codeowls.ScriptProvider.nodes.scriptfolder -arg $name,$script,$idField,$script:currentParentNode.Peek();

    return $s;
}

function folder {
 
    param(
        [parameter(position=0, mandatory=$true)]
        [string]
        $name,

        [parameter(position=1, mandatory=$true)]
        [scriptblock]
        $script
    )

    $folder = new-object codeowls.ScriptProvider.nodes.folder -arg $name,$script:currentParentNode.Peek();
    $script:currentParentNode.Push($folder);
    $items=[codeowls.ScriptProvider.nodes.iitem[]]@(& $script); 
    $folder.Children.AddRange( $items );
    $script:currentParentNode.Pop() | out-null;

    return $folder;
}