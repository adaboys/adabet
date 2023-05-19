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

class RememberReferrerUser
{
    
    public function handle($request, Closure $next)
    {
        $response = $next($request);

        
        if ($request->is('/') && !$request->hasCookie('ref') && $request->query('ref') ) {
            
            $response->cookie('ref', $request->query('ref'), 525600);
        }

        return $response;
    }
}
