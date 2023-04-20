<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Repositories;

use App\Facades\AccountTransaction;
use App\Models\Account;
use App\Models\Bonus;

class BonusRepository
{
    
    public static function create(Account $account, int $type, float $amount): ?Bonus
    {
        if ($amount == 0) {
            return NULL;
        }

        
        $bonus = new Bonus();
        $bonus->account()->associate($account);
        $bonus->type = $type;
        $bonus->amount = $amount;
        $bonus->save();

        AccountTransaction::create($account, $bonus, $amount);

        return $bonus;
    }
}
