<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Http\Middleware;

use App\Models\MultiplayerGame;
use Closure;
use Illuminate\Support\Facades\DB;

class LockMultiplayerGame
{
    
    public function handle($request, Closure $next)
    {
        
        DB::beginTransaction();

        
        
        $multiplayerGame = $request->route('multiplayerGame');
        if ($multiplayerGame instanceof MultiplayerGame) {
            $multiplayerGame->gameable()->lockForUpdate()->get();
        }

        
        $response = $next($request);

        
        
        DB::commit();

        return $response;
    }
}
