ipmo ./scriptprovider.psm1
ipmo sqlite

mount-sqlite tmpdb;
new-item tempdb:/mydata -ItemName TEXT -ItemCount INTEGER
0..19 | foreach {
    new-item tempdb:/mydata -itemname "Item #$_" -itemcount (get-random);
}

root {

    folder Data {
        script DB {

        }
    }
        
    folder System {
        
        script "ApplicationLog" -id Index {
            get-eventlog -log application -newest 100 
        } 
        
        script "PowerShellLog" -id Index {
            get-eventlog -log *powershell -newest 100 
        }                
        
        
        script Processes -id ID {
            get-process
        }  
        
    }
    
}