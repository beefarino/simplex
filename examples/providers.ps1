<#
This example demonstrates how you can "proxy" other providers through
your simplex provider.

In this example, the /docs folder will contain a collection of files
and folders depending on the contents of your $home folder.  You can interact
with these files and folders as if they were part of the simplex provider.
#>

root {

  script "docs" {

    dir $home\documents

  }

}
