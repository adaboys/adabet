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

use App\Helpers\PackageManager;
use App\Rules\BalanceIsSufficient;
use Illuminate\Foundation\Http\FormRequest;

class MakeAssetPrediction extends FormRequest
{
    protected $gameableClass;

    
    public function authorize()
    {
        return $this->asset->is_active;
    }

    
    public function rules()
    {
        $packageManager = app()->make(PackageManager::class);
        $packageId = $packageManager->getPackageIdByClass($this->gameableClass);

        return [
            'bet' => [
                'required',
                'integer',
                'min:' . config($packageId . '.min_bet'),
                'max:' . config($packageId . '.max_bet'),
            ],
            'direction' => 'required|in:-1,1',
            'duration' => 'required|in:' .  collect(config($packageId . '.durations'))
                    ->pluck('value')
                    ->implode(','),
        ];
    }

    public function withValidator($validator)
    {
        $user = $this->user();
        $amount = $this->bet;

        
        $validator->after(function ($validator) use ($user, $amount) {
            $rule = new BalanceIsSufficient($user, $amount);

            if (!$rule->passes()) {
                $validator->errors()->add('bet', $rule->message());
            }
        });
    }
}
