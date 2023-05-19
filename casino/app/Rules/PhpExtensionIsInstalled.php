<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Rules;

use Illuminate\Contracts\Validation\Rule;

class PhpExtensionIsInstalled implements Rule
{
    private $extension;

    
    public function __construct(string $extension)
    {
        $this->extension = $extension;
    }

    
    public function passes($attribute = NULL, $value = NULL)
    {
        return extension_loaded($this->extension);
    }

    
    public function message()
    {
        return __('PHP extension ":ext" should be installed and enabled on your server.', ['ext' => $this->extension]);
    }
}
