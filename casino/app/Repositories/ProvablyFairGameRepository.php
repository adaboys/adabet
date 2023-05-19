<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */


namespace App\Repositories;

use App\Models\ProvablyFairGame;
use Illuminate\Support\Carbon;


class ProvablyFairGameRepository
{
    public static function search($hash, $gameableType): ?ProvablyFairGame
    {
        return ProvablyFairGame::where('hash', $hash)
            ->where('gameable_type', $gameableType)
            ->where('created_at', '>=', Carbon::now()->subDay())
            ->first();
    }
}
