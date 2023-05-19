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

use App\Rules\FaucetIsAllowed;
use Illuminate\Foundation\Http\FormRequest;

class ClaimFaucet extends FormRequest
{
    
    public function authorize()
    {
        return TRUE;
    }

    
    public function rules()
    {
        return [];
    }

    public function withValidator($validator)
    {
        $user = $this->user();

        $validator->after(function ($validator) use ($user) {
            $rule = new FaucetIsAllowed($user);

            if (!$rule->passes()) {
                $validator->errors()->add('amount', $rule->message());
            }
        });
    }
}
