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
use Illuminate\Foundation\Http\FormRequest;

class GetMultiplayerGame extends FormRequest
{
    
    public function authorize(PackageManager $packageManager)
    {
        $package = $packageManager->get($this->packageId);

        return optional($package)->enabled;
    }

    
    public function rules()
    {
        return [
            
        ];
    }
}
