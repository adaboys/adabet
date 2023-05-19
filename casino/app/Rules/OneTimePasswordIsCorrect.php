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
use PragmaRX\Google2FA\Google2FA;

class OneTimePasswordIsCorrect implements Rule
{
    private $secret;

    
    public function __construct(?string $secret)
    {
        $this->secret = $secret;
    }

    
    public function passes($attribute, $value)
    {
        $google2fa = new Google2FA();
        return $value && $this->secret && $google2fa->verifyKey($this->secret, $value);
    }

    
    public function message()
    {
        return __('Incorrect one-time password.');
    }
}
