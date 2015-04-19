<#
This example demonstrates how you can generate dynamic provider nodes.

In addition, notice the use of GetNewClosure() to capture the values
of external variables referenced inside of the scriptblocks used to
define simplex provider nodes.
#>

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
