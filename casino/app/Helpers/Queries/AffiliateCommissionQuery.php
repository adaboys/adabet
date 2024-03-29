<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   AffiliateCommissionQuery.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Helpers\Queries;

use App\Helpers\Queries\Filters\AffiliateCommissionStatusFilter;
use App\Helpers\Queries\Filters\AffiliateCommissionTypeFilter;
use App\Helpers\Queries\Filters\PeriodFilter;
use App\Models\AffiliateCommission;

class AffiliateCommissionQuery extends Query
{
    protected $model = AffiliateCommission::class;

    protected $with = ['account', 'account.user', 'account.user.profiles', 'account.user.referrer'];

    protected $filters = [[PeriodFilter::class, AffiliateCommissionTypeFilter::class, AffiliateCommissionStatusFilter::class]];

    protected $searchableColumns = [['id'], 'account.user' => ['name', 'email']];

    protected $sortableColumns = [
        'id'                => 'id',
        'tier'              => 'tier',
        'amount'            => 'amount',
        'created_at'        => 'created_at',
        'created_ago'       => 'created_at',
    ];
}
