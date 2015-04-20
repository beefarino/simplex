root {

    script "Outer" {

        $name = "Inner"
        script "$name" {
            get-process
        }
    }
}