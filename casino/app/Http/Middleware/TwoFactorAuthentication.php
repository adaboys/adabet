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
use Illuminate\Support\Facades\Redirect;
use Illuminate\Support\Facades\Route;

class TwoFactorAuthentication
{
    
    public function handle($request, Closure $next)
    {
        $user = $request->user();
        $route = Route::currentRouteName();

        
        if ($user->two_factor_auth_enabled
            && !$user->two_factor_auth_passed
            && !in_array($route, ['user', 'user.security.2fa.verify'])) {
            return $request->expectsJson()
                ? abort(HTTP_CODE_2FA_NOT_PASSED)
                : Redirect::to('/2fa');
        }

        return $next($request);
    }
}
