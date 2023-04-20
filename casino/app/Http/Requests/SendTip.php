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

use App\Rules\BalanceIsSufficient;
use App\Rules\TipDoesNotExceedMaxAllowedAmount;
use Illuminate\Foundation\Http\FormRequest;

class SendTip extends FormRequest
{
    
    public function authorize()
    {
        return config('settings.tips.enabled')
            && $this->user()->id != $this->user->id
            && $this->user()->is_active
            && $this->user->is_active;
    }

    
    public function rules()
    {
        return [
            'amount' => 'required|integer|min:1',
        ];
    }

    public function withValidator($validator)
    {
        $user = $this->user();
        $amount = $this->amount;

        $validator->after(function ($validator) use ($user, $amount) {
            $balanceRule = new BalanceIsSufficient($user, $amount);
            $tipRule = new TipDoesNotExceedMaxAllowedAmount($user);

            if (!$balanceRule->passes()) {
                $validator->errors()->add('amount', $balanceRule->message());
            } elseif (!$tipRule->passes(NULL, $amount)) {
                $validator->errors()->add('amount', $tipRule->message());
            }
        });
    }
}
