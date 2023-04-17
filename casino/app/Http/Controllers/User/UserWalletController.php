<?php

namespace App\Http\Controllers\User;

use App\Http\Controllers\Controller;
use App\Http\Requests\User\Depositrequest;
use App\Services\UserWalletService;
use Http\Adapter\Guzzle7\Client;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Log;

class UserWalletController extends Controller
{
    protected $service;

    public function __construct(UserWalletService $service)
    {
        $this->service = $service;
    }

    public function deposit(Depositrequest $request)
    {
        $user = $request->user();
        $wallet = $user->wallet()->where('is_primary', 1)->first();
        $response = $this->service->deposit($wallet->address, $request->input('amount'));

        if ($response->status== 200 && $response->data->tx_id) {
            $this->service->convertAdaToChip($user, $request->input('amount'));
            return response()->json([
                'message' => $response->message,
                'status' => true
            ]);
        }

        return response()->json([
            'message' => $response->message,
            'status' => false
        ]);
    }
}
