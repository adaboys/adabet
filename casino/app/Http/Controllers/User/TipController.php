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

use App\Facades\AccountTransaction;
use App\Http\Requests\SendTip;
use App\Http\Controllers\Controller;
use App\Models\GenericAccountTransaction;
use App\Models\User;

class TipController extends Controller
{
    public function send(SendTip $request, User $user)
    {
        AccountTransaction::createGeneric($request->user()->account, GenericAccountTransaction::TYPE_TIP, -$request->amount);
        AccountTransaction::createGeneric($user->account, GenericAccountTransaction::TYPE_TIP, $request->amount);

        return $this->successResponse(['user' => $request->user()->loadMissing('account')]);
    }
}
