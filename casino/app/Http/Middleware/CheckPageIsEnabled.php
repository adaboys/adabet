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

class CheckPageIsEnabled
{
    
    public function handle($request, Closure $next)
    {
        if (!config('settings.content.leaderboard.enabled') && $request->is('api/leaderboard')) {
            return response()->json(['error' => __('Forbidden')], 403);
        }

        return $next($request);
    }
}
