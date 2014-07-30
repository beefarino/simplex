root {
   folder System {
       script Processes -id id {
           get-process
	   };

	   script Errors -id index {
	       get-eventlog -log application -eventtype error -newest 50
	   };
   };

   folder Generated {
	   0..9 | foreach-object {
	       script "Generated$_" {
		       $_
		   };
	   }
	};
}