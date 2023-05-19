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
use Illuminate\Support\Carbon;

class CompleteAssetPrediction extends FormRequest
{
    
    public function authorize()
    {
        return $this->game->is_in_progress
            && $this->game->account->id == $this->user()->account->id
            && $this->game->gameable->end_time->lte(Carbon::now());
    }

    
    public function rules()
    {
        return [];
    }
}
