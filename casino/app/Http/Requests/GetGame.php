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

class GetGame extends FormRequest
{
    
    public function authorize()
    {
        return $this->game && $this->game->is_completed;
    }

    
    public function rules()
    {
        return [
            
        ];
    }
}
