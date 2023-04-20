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

class GetAssetData extends FormRequest
{
    
    public function authorize()
    {
        return $this->asset->is_active;
    }

    
    public function rules()
    {
        return [
            
        ];
    }
}
