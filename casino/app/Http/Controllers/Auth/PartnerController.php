<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use App\Http\Requests\RegisterFromPartner;
use App\Repositories\UserRepository;
use App\Traits\ApiResponse;
use Illuminate\Http\Request;

class PartnerController extends Controller
{
    use ApiResponse;
    public function register(RegisterFromPartner $request)
    {
        UserRepository::create($request->all());

        return $this->httpCreated();
    }

    public function buildSignature(Request $request)
    {
        $apiKey = config('settings.partner.api_key');
        $signature = hash_hmac('sha256', json_encode($request->all()), $apiKey);

        $this->httpOk(['generate signature success full', 'signature' => $signature], true);
    }
}
