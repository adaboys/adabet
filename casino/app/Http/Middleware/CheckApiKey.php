<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Laravel\SerializableClosure\Signers\Hmac;

class CheckApiKey
{
    /**
     * Handle an incoming request.
     *
     * @param  \Illuminate\Http\Request  $request
     * @param  \Closure(\Illuminate\Http\Request): (\Illuminate\Http\Response|\Illuminate\Http\RedirectResponse)  $next
     *
     * @return \Illuminate\Http\JsonResponse
     */
    public function handle(Request $request, Closure $next)
    {
        $apiKey = config('settings.partner.api_key');

        if (hash_hmac('sha256', json_encode($request->all()), $apiKey) != $request->bearerToken()) {
            return response()->json([
                'message' => 'api key not matching',
                'status' => false
            ], 403);
        }
        return $next($request);
    }
}
