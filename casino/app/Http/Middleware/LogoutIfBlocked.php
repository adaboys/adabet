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

use App\Models\User;
use Closure;

class LogoutIfBlocked
{
    
    public function handle($request, Closure $next)
    {
        
        if (!$request->user()->is_active) {
            auth()->guard('web')->logout();
            abort(401);
        }

        return $next($request);
    }
}
