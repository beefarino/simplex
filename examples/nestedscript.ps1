<#
This example demonstrates the ability to nest script nodes inside of one
another.  The "Outer" script node contains two script nodes, the first of which
uses the local script state to define the folder name, the latter of which
renames the folder every second.
#>

root {

    script "Outer" {

        $name = "Inner"
        script "$name" {
            get-process
        }

        script ((get-date).second) {
          get-psprovider
        }
    }
}
