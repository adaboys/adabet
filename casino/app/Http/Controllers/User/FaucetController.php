<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Http\Controllers\User;

use App\Http\Requests\ClaimFaucet;
use App\Models\Bonus;
use App\Repositories\BonusRepository;
use App\Rules\FaucetIsAllowed;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;
use Illuminate\Support\Carbon;

class FaucetController extends Controller
{
    public function show(Request $request)
    {
        $rule = new FaucetIsAllowed($request->user());

        return [
            'allowed'   => $rule->passes(),
            'time'      => $rule->getAllowedTime()->getTimestamp()
        ];
    }

    public function update(ClaimFaucet $request)
    {
        return [
            'success'   => !!BonusRepository::create($request->user()->account, Bonus::TYPE_FAUCET, config('settings.bonuses.faucet.amount')),
            'time'      => Carbon::now()->addHours(config('settings.bonuses.faucet.interval'))->getTimestamp()
        ];
    }
}
