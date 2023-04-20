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

class EnsureEmailIsVerified
{
    
    public function handle($request, Closure $next, $redirectToRoute = null)
    {
        
        if (config('settings.users.email_verification') && !$request->user()->hasVerifiedEmail()) {
            return $request->expectsJson()
                ? abort(HTTP_CODE_EMAIL_NOT_VERIFIED)
                : Redirect::to('/email');
        }

        return $next($request);
    }
}
