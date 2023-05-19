<?php

namespace App\Http\Middleware;

use App\Models\User;
use Closure;
use GuzzleHttp\Client;
use Illuminate\Http\Request;
use GuzzleHttp\Psr7\Request as ClientRequest;
use Illuminate\Support\Facades\Auth;

class SignOnAdabetMiddleware
{
    /**
     * Handle an incoming request.
     *
     * @param  \Illuminate\Http\Request  $request
     * @param  \Closure(\Illuminate\Http\Request): (\Illuminate\Http\Response|\Illuminate\Http\RedirectResponse)  $next
     * @return \Illuminate\Http\Response|\Illuminate\Http\RedirectResponse
     */
    public function handle(Request $request, Closure $next)
    {
        $accessToken = $_COOKIE['adabet-refresh-token'] ?? '';
        $checkForceLogin = $_COOKIE['singed'] ?? false;
        if (!$accessToken || $checkForceLogin) {
            return $next($request);
        }

        $adabetUserInfo = $this->getUserInfo($accessToken);
        if(!Auth::check() && $adabetUserInfo && $adabetUserId = $adabetUserInfo->data->user_id) {
            $userId = User::query()->where(['user_id' => $adabetUserId, 'status' => User::STATUS_ACTIVE])->pluck('id')->first();

            if ($userId) {
                Auth::loginUsingId($userId);
                setcookie('singed', true, time() + (60 * 60 * 24), '/', config('session.domain'), false, true);
            }
        }
        return $next($request);
    }

    protected function getUserInfo($accessToken)
    {
        $client = new Client();
        $headers = [
            'Content-Type' => 'application/json',
            'X-Requested-With' => 'XMLHttpRequest',
        ];

        $url = config('settings.partner.adabet_s1_api_domain') . '/api/s1/user/auth_info?api_key=' . config('settings.partner.api_key') .'&refresh_token=' . $accessToken;

        $request = new ClientRequest('GET', $url, $headers);
        $response = $client->sendAsync($request)->wait();
        return json_decode($response->getBody()->getContents());
    }
}
