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

use App\Models\GenericAccountTransaction;
use App\Models\User;
use Illuminate\Contracts\Validation\Rule;
use Illuminate\Support\Carbon;

class TipDoesNotExceedMaxAllowedAmount implements Rule
{
    protected $config;
    protected $user;
    protected $pastTipAmount;

    
    public function __construct(User $user)
    {
        $this->config = config('settings.tips');

        $this->user = $user;

        $this->pastTipAmount = abs(GenericAccountTransaction::where('type', GenericAccountTransaction::TYPE_TIP)
            ->where('account_id', $this->user->account->id)
            ->where('amount', '<', 0)
            ->where('created_at', '>=', Carbon::now()->subHours($this->config['interval']))
            ->sum('amount'));
    }

    
    public function passes($attribute, $value)
    {
        return $value + $this->pastTipAmount <= $this->config['max_amount'];
    }

    
    public function message()
    {
        return __('You can not tip more than :x during every :h hours (already tipped :x2)', [
            'x' => $this->config['max_amount'],
            'x2' => $this->pastTipAmount,
            'h' => $this->config['interval']
        ]);
    }
}
