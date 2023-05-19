<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   MagicGetters.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */


namespace App\Helpers;

use Illuminate\Support\Str;

trait MagicGetters
{
    public function __get($name)
    {
        $getter = 'get' . Str::studly($name);

        if (!method_exists($this, $getter)) {
            throw new \Exception(sprintf('Method %s does not exist in class %s.', $getter, __CLASS__));
        }

        return call_user_func([$this, $getter]);
    }
}
