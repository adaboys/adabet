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

use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Carbon;

class UpdateLastSeen
{
    
    public function handle(Request $request, Closure $next)
    {
        $user = $request->user();

        
        
        if ($user && (is_null($user->last_seen_at) || $user->last_seen_at->lte(Carbon::now()->subSeconds(5)))) {
            tap($request->user(), function ($user) { $user->is_online = TRUE; })->save();
        }

        return $next($request);
    }
}
