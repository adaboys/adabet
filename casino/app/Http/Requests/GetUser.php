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

class GetUser extends FormRequest
{
    
    public function authorize()
    {
        
        return $this->user && ($this->user->is_active || $this->user()->is_admin);
    }

    
    public function rules()
    {
        return [
            
        ];
    }
}
