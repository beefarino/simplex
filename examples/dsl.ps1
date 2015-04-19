<#
This example demonstrates the basic DSL features you need to know
to create a Simplex provider.


#>
root {

    folder Events {

        script Errors {
            get-eventlog -LogName Application -Newest 20 -EntryType error;
        }

        script Warnings {
            get-eventLog -LogName Application -newest 20 -EntryType Warning;
        }

        script Infos {
            get-eventLog -LogName Application -newest 20 -EntryType Information;
        }
    }
}
