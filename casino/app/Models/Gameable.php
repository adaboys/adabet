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

trait Gameable
{
    use StandardDateFormat;

    
    public function game(): MorphOne
    {
        return $this->morphOne(Game::class, 'gameable');
    }

    
    public function games(): MorphMany
    {
        return $this->morphMany(Game::class, 'gameable');
    }

    
    public function getIsProvablyFairAttribute(): bool
    {
        return $this instanceof ProvableGame;
    }
}
