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

class CheckPermissions
{
    
    public function handle(Request $request, Closure $next)
    {
        $user = $request->user();
        $module = $request->segment(3);

        if (($request->getMethod() == 'GET' && !$user->hasReadOnlyAccess($module)) || ($request->getMethod() != 'GET' && !$user->hasFullAccess($module))) {
            abort(403, __('You do not have permissions to complete this operation.'));
        }

        return $next($request);
    }
}
