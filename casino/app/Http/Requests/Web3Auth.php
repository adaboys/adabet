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

use Illuminate\Foundation\Http\FormRequest;

class Web3Auth extends FormRequest
{
    
    public function authorize()
    {
        return $this->provider
            && array_key_exists($this->provider, config('auth.web3'))
            && config('auth.web3.' . $this->provider . '.enabled');
    }

    
    public function rules()
    {
        return [
            'signature' => 'required|string|min:20',
            'address' => 'required|string|min:20',
        ];
    }
}
