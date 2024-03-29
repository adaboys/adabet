<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Rules;

use App\Models\Bonus;
use App\Models\User;
use Illuminate\Contracts\Validation\Rule;
use Illuminate\Support\Carbon;

class FaucetIsAllowed implements Rule
{
    protected $config;
    protected $user;
    protected $faucet;

    
    public function __construct(User $user)
    {
        $this->config = config('settings.bonuses.faucet');

        $this->user = $user;

        $this->faucet = Bonus::where('type', Bonus::TYPE_FAUCET)
            ->where('account_id', $this->user->account->id)
            ->orderBy('created_at', 'desc')
            ->first();
    }

    
    public function passes($attribute = NULL, $value = NULL)
    {
        return $this->config['amount'] > 0
            && (!$this->faucet || $this->faucet->created_at->lt(Carbon::now()->subHours($this->config['interval'])));
    }

    public function getAllowedTime(): Carbon
    {
        return optional($this->faucet)->created_at
            ? $this->faucet->created_at->addHours($this->config['interval'])
            : Carbon::now();
    }

    
    public function message()
    {
        return $this->config['amount'] > 0
            ? __('You can claim faucet in :time', ['time' => $this->getAllowedTime()->diffForHumans()])
            : __('Faucet is disabled.');
    }
}
