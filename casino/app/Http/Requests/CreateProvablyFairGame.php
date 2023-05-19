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

class CreateProvablyFairGame extends FormRequest
{
    
    public function authorize()
    {
        return TRUE;
    }

    
    public function rules()
    {
        return [
            'game_package_id'   => 'required', 
            'client_seed'       => 'required|max:32',
        ];
    }
}
