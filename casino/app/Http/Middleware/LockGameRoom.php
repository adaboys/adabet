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

use App\Models\GameRoom;
use Closure;
use Illuminate\Support\Facades\DB;

class LockGameRoom
{
    
    public function handle($request, Closure $next)
    {
        
        DB::beginTransaction();

        
        $roomId = (int) $request->room_id ?: ($request->room && $request->room instanceof GameRoom ? $request->room->id : 0);

        
        
        if ($roomId){
            GameRoom::where('id', $roomId)->lockForUpdate()->first();
        }

        
        $response = $next($request);

        
        
        DB::commit();

        return $response;
    }
}
