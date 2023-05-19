<?php

namespace App\Services;

use App\Models\Bonus;
use App\Repositories\BonusRepository;
use GuzzleHttp\Client;
use GuzzleHttp\Psr7\Request;
use Illuminate\Support\Facades\Http;

class UserWalletService
{

    public function __construct()
    {

    }

    /**
     * @param $senderAddress
     * @param $totalAda
     *
     * @return \Psr\Http\Message\ResponseInterface
     */
    public function deposit($senderAddress, $totalAda)
    {
        $url = config('wallet.deposit_domain') . config('wallet.deposit_endpoint');
        $headers = [
            'Authorization' => 'api_key ' . config('wallet.api_key'),
            'Content-Type' => 'application/json'
        ];

        $body = $this->buildParamsDeposit($senderAddress, $totalAda);
        $client = new Client();

        $request = new Request('POST', $url, $headers, json_encode($body));
        $response = $client->sendAsync($request)->wait();

        return json_decode($response->getBody()->getContents());
    }

    public function convertAdaToChip($user, $totalAda)
    {
        $chip = $totalAda * config('wallet.ada_to_chip');
        BonusRepository::create($user->account, Bonus::TYPE_DEPOSIT, $chip);
    }

    /**
     * @param $senderAddress
     * @param $totalAda
     *
     * @return array
     */
    protected function buildParamsDeposit($senderAddress, $totalAda): array
    {
        return [
            'site_id' => 'adabet',
            'sender_address' => $senderAddress,
            'receiver_address' => config('wallet.receiver_address'),
            'fee_payer_address' => $senderAddress,
            'discount_fee_from_assets' => false,
            'force_send_all_assets' => false,
            'assets' => [
                [
                    'asset_id' => 'lovelace',
                    'quantity' => (string)($totalAda * pow(10, 6)),
                ]
            ],
        ];
    }
}