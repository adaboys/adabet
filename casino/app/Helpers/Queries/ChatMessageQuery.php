<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   ChatMessageQuery.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */
namespace App\Helpers\Queries;

use App\Helpers\Queries\Filters\PeriodFilter;
use App\Models\ChatMessage;

class ChatMessageQuery extends Query
{
    protected $model = ChatMessage::class;

    protected $with = ['room', 'user', 'user.profiles', 'user.referrer'];

    protected $filters = [[PeriodFilter::class]];

    protected $searchableColumns = [['id', 'message'], 'user' => ['name', 'email']];

    protected $sortableColumns = [
        'id'            => 'id',
        'created_ago'   => 'created_at',
    ];
}
