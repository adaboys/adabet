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

use App\Rules\PasswordIsCorrect;
use Illuminate\Foundation\Http\FormRequest;

class UpdatePassword extends FormRequest
{
    
    public function authorize()
    {
        return true;
    }

    
    public function rules()
    {
        

        return [
            'current_password' => [
                'required',
                new PasswordIsCorrect($this->user())
            ],
            'password'              => 'required|confirmed|min:6',
            'password_confirmation' => 'required'
        ];
    }
}
