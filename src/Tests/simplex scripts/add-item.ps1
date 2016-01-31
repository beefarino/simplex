root {
  folder "Outer" {
    $script:r = new-object system.collections.arraylist;
    $script:r.addRange( 0..9 );
    script 'TestContainer' {
      return $script:r;
    } -add {
      $script:r.add($args[0]);
    } -remove {
      $script:r.remove($args[0]);
    }
  }
}
