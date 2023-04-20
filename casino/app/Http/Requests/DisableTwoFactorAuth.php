<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Http\Requests;

use App\Rules\OneTimePasswordIsCorrect;
use App\Rules\PasswordIsCorrect;
use Illuminate\Foundation\Http\FormRequest;

class DisableTwoFactorAuth extends FormRequest
{
    
    public function authorize()
    {
        
        return $this->user()->two_factor_auth_enabled;
    }

    
    public function rules()
    {
        $user = $this->user()->loadMissing('profiles');

        return [
            'one_time_password' => [
                'required',
                new OneTimePasswordIsCorrect($user->totp_secret)
            ],
            
            'password' => $user->profiles->isEmpty()
                ? ['required', new PasswordIsCorrect($user)]
                : ''
        ];
    }
}
