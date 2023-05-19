<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   AffiliateCommissionTypeFilter.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */
namespace App\Helpers\Queries\Filters;

use App\Models\AffiliateCommission;

class AffiliateCommissionTypeFilter extends EnumFilter
{
    protected $key = 'type';
    protected $model = AffiliateCommission::class;
    protected $table = 'affiliate_commissions';
}
