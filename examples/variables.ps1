
function get-applicationEvents
{
    param ( $entryType );

    get-eventlog -LogName Application -newest 20 -EntryType $entryType
}

root {

    folder Events {

        $types = 'error','warning','information'

        $types | foreach-object {
            
            $type = $_
            
            script "${type}s" {
                get-applicationEvents -EntryType $type;
            }.GetNewClosure()
        }
    }
}
