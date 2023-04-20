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

use GuzzleHttp\Client;
use Illuminate\Contracts\Validation\Rule;
use Illuminate\Support\Facades\Log;

class ReCaptchaValidationPassed implements Rule
{
    
    public function passes($attribute, $value)
    {
        
        $client = new Client([
            'base_uri' => 'https://google.com/recaptcha/api/'
        ]);
        $response = $client->post('siteverify', [
            'query' => [
                'secret'    => config('services.recaptcha.secret_key'),
                'response'  => $value,
                'remoteip'  => request()->ip(),
            ]
        ]);

        Log::info($response->getBody());

        $responseBody = json_decode($response->getBody());

        return $responseBody->success ?? FALSE;
    }

    
    public function message()
    {
        return __('Please verify that you are a human.');
    }
}
