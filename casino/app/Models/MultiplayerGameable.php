<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Models;

use Illuminate\Database\Eloquent\Relations\MorphMany;
use Illuminate\Database\Eloquent\Relations\MorphOne;

trait MultiplayerGameable
{
    use StandardDateFormat;

    
    public function multiplayerGame(): MorphOne
    {
        return $this->morphOne(MultiplayerGame::class, 'gameable');
    }
}
