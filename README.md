# Simplex

A powershell module used to create powershell providers using a simple DSL.

# Example Usage

This example assumes the following Simplex script file is available at c:\share\mydrive.ps1:

## Simplex Script

```powershell
root {
   folder MyFolder {
       script MyEnvironment -id key {
           dir env:
	   }
	   script MyModules -id guid {
			get-module
	   }
   }
}
```

## PowerShell Session

```powershell
import-module simplex
new-psdrive s -psprovider simplex -root "c:\share\mydrive.ps1"
cd s:\myfolder\mymodules
dir
```

# Simplex SDL

The provider DSL consists of three elements:

* root - used to define the root of the drive tree; the root may contain any number of folder and/or script elements
* folder - used to define simple named containers on the drive; the folder may contain any number of folder and/or script elements
* script - used to define a named collection of items; the script element uses a PowerShell script to return the items it contains; you can specify a property name use as the child name of the item

## Example: Canonical Drive

```powershell
root {
   folder MyFolder {
       script MyEnvironment -id key {
           dir env:
	   }
	   script MyModules -id guid {
			get-module
	   }
   }
}
```

## Example: Generated Drive Content

```powershell
root {
   folder System {
       script Processes -id id {
           get-process
	   }

	   script Errors -id index {
	       get-eventlog -log application -eventtype error -newest 50
	   }
   }

   folder Generated {
	   0..9 | foreach-object {
	       script "Generated$_" {
		       $_
		   }
	   }
	}
}
```

