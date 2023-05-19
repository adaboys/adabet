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

class CheckSocialProvider
{
    
    public function handle($request, Closure $next)
    {
        if (!config('oauth.'.$request->provider.'.client_id')
            || !config('oauth.'.$request->provider.'.client_secret')) {
            return redirect()->route('login');
        }

        return $next($request);
    }
}
